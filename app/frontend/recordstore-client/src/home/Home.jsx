import { useEffect, useState } from 'react';
import {
  Center,
  Heading,
  Text,
  Card,
  CardHeader,
  CardBody,
  CardFooter,
  Grid,
  GridItem,
  Image,
  LinkBox,
  LinkOverlay,
  CircularProgress,
  Flex,
  Box,
  Input,
  RangeSlider,
  RangeSliderTrack,
  RangeSliderFilledTrack,
  RangeSliderThumb,
  Button,
} from '@chakra-ui/react';

import {
  AutoComplete,
  AutoCompleteInput,
  AutoCompleteItem,
  AutoCompleteList,
} from '@choc-ui/chakra-autocomplete';

import { Link as ReactRouterLink } from 'react-router-dom';
import productService from './ProductService';
import genreService from './GenreService';
import formatService from './FormatService';

const Home = () => {
  const [products, setProducts] = useState([]);
  const [search, setSearch] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const [title, setTitle] = useState('');
  const [artist, setArtist] = useState('');
  const [genre, setGenre] = useState('');
  const [format, setFormat] = useState('');
  const [minPrice, setMinPrice] = useState(0);
  const [maxPrice, setMaxPrice] = useState(0);
  const [page, setPage] = useState(1);

  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);

  const [prices, setPrices] = useState({});

  useEffect(() => {
    const fetchPrices = async () => {
      const prices = await productService.getMinMaxPrices();
      setPrices(prices);
    };

    fetchPrices();
  }, []);

  useEffect(() => {
    const fetchProducts = async () => {
      const filterParams = {};

      if (title) {
        filterParams.Title = title;
      }
      if (title !== '') {
        filterParams.Title = title;
      }
      if (artist !== '') {
        filterParams.Artist = artist;
      }
      if (genre !== '') {
        filterParams.Genre = genre;
      }
      if (format !== '') {
        filterParams.Format = format;
      }
      if (minPrice !== 0) {
        filterParams.MinPrice = minPrice;
      }
      if (maxPrice !== 0) {
        filterParams.MaxPrice = maxPrice;
      }
      filterParams.Page = page;

      const products = await productService.getProducts(filterParams);
      setProducts(products.results);
      setTotalPages(products.pageCount);
      setTotalCount(products.rowCount);
    };

    setIsLoading(true);
    fetchProducts();
    setIsLoading(false);
  }, [search]);

  const [genreOptions, setGenreOptions] = useState([]);
  const [genreSearch, setGenreSearch] = useState('');
  const [isGenreLoading, setIsGenreLoading] = useState(false);

  const [formatOptions, setFormatOptions] = useState([]);
  const [formatSearch, setFormatSearch] = useState('');
  const [isFormatLoading, setIsFormatLoading] = useState(false);

  useEffect(() => {
    const fetchFormats = async () => {
      const formats = await formatService.getFormats(formatSearch);
      console.log(formats);
      setFormatOptions(formats);
    };

    setIsFormatLoading(true);
    fetchFormats();
    setIsFormatLoading(false);
  }, [formatSearch]);

  useEffect(() => {
    const fetchGenres = async () => {
      const genres = await genreService.getGenres(genreSearch);
      console.log(genres);
      setGenreOptions(genres);
    };

    setIsGenreLoading(true);
    fetchGenres();
    setIsGenreLoading(false);
  }, [genreSearch]);

  const handleSearch = () => {
    setSearch(!search);
  };

  return (
    <>
      {isLoading ? (
        <Center>
          <CircularProgress isIndeterminate />
        </Center>
      ) : (
        <>
          <Text mt={4} ms={4}>
            Знайдено {totalCount} записів.
          </Text>
          <Flex direction='row' pt={10}>
            <Box w='20%'>
              <Box borderWidth={1} p={4}>
                <Heading as='h3' size='md' mb={2}>
                  Назва
                </Heading>
                <Flex direction='row'>
                  <Input
                    type='text'
                    placeholder='Введіть назву'
                    value={title}
                    onChange={(e) => {
                      setTitle(e.target.value);
                      handleSearch();
                    }}
                  />
                </Flex>
              </Box>
              <Box borderWidth={1} p={4} mb={4}>
                <Heading as='h3' size='md' mb={2}>
                  Виконавець
                </Heading>
                <Flex direction='row'>
                  <Input
                    type='text'
                    placeholder='Введіть виконавця'
                    value={artist}
                    onChange={(e) => {
                      setArtist(e.target.value);
                      handleSearch();
                    }}
                  />
                </Flex>
              </Box>

              <Box borderWidth={1} p={4} mb={4}>
                <Heading as='h3' size='md' mb={2}>
                  Формат
                </Heading>
                <AutoComplete openOnFocus isLoading={isGenreLoading}>
                  <AutoCompleteInput
                    onChange={(e) => {
                      if (e.target.value === '') {
                        setFormat('');
                        handleSearch();
                      }
                      setFormatSearch(e.target.value);
                    }}
                  />
                  <AutoCompleteList>
                    {formatOptions.map((option) => (
                      <AutoCompleteItem
                        key={option.name}
                        onClick={() => {
                          setFormat(option.name);
                          handleSearch();
                        }}
                        value={option.name}
                      >
                        {option.name}
                      </AutoCompleteItem>
                    ))}
                  </AutoCompleteList>
                </AutoComplete>
              </Box>
              <Box borderWidth={1} p={4} mb={4}>
                <Heading as='h3' size='md' mb={2}>
                  Жанр
                </Heading>
                <AutoComplete openOnFocus isLoading={isGenreLoading}>
                  <AutoCompleteInput
                    onChange={(e) => {
                      if (e.target.value === '') {
                        setGenre('');
                        handleSearch();
                      }
                      setGenreSearch(e.target.value);
                    }}
                  />
                  <AutoCompleteList>
                    {genreOptions.map((option) => (
                      <AutoCompleteItem
                        key={option.name}
                        onClick={() => {
                          setGenre(option.name);
                          handleSearch();
                        }}
                        value={option.name}
                      >
                        {option.name}
                      </AutoCompleteItem>
                    ))}
                  </AutoCompleteList>
                </AutoComplete>
              </Box>
              <Box borderWidth={1} p={4} mb={4}>
                <Heading as='h3' size='md' mb={2}>
                  Ціна
                </Heading>
                <Flex direction='row' justifyContent='space-between'>
                  <Text>{minPrice} ₴</Text>
                  <Text>{maxPrice} ₴</Text>
                </Flex>
                <RangeSlider
                  min={prices.minPrice}
                  max={prices.maxPrice}
                  step={1}
                  defaultValue={[minPrice, maxPrice]}
                  onChange={(values) => {
                    setMinPrice(values[0]);
                    setMaxPrice(values[1]);
                  }}
                >
                  <RangeSliderTrack>
                    <RangeSliderFilledTrack />
                  </RangeSliderTrack>
                  <RangeSliderThumb index={0} />
                  <RangeSliderThumb index={1} />
                </RangeSlider>
                <Button
                  mt={4}
                  colorScheme='blue'
                  onClick={() => {
                    handleSearch();
                  }}
                >
                  ОК
                </Button>
              </Box>
            </Box>
            <Grid templateColumns='repeat(5, 1fr)' gap={6} w='80%' px={15}>
              {products.length === 0 && (
                <Center>
                  <Text>Нічого не знайдено</Text>
                </Center>
              )}
              {products.map((product, index) => (
                <GridItem key={index}>
                  <LinkBox as='article'>
                    <Card
                      _hover={{ boxShadow: '0 4px 8px 0 rgba(0,0,0,0.2)' }}
                      maxW='md'
                    >
                      <Center>
                        <Image
                          src={product.image_url}
                          fallbackSrc='https://via.placeholder.com/150'
                          alt='product image'
                          boxSize={250}
                          objectFit='cover'
                        />
                      </Center>
                      <LinkOverlay
                        as={ReactRouterLink}
                        to={`/product/${product.id}`}
                      >
                        <CardHeader>
                          <Heading as='h4' size='sm' fontWeight='semibold'>
                            {product.title}
                          </Heading>
                          <Text fontSize='sm' mt={4} color='gray.500'>
                            {product.format.name}
                          </Text>
                        </CardHeader>
                      </LinkOverlay>
                      <CardBody>
                        {product.artists.map((artist) =>
                          artist.name === product.artists[0].name
                            ? artist.name
                            : `, ${artist.name}`
                        )}
                      </CardBody>
                      <CardFooter>{product.price} ₴</CardFooter>
                    </Card>
                  </LinkBox>
                </GridItem>
              ))}
            </Grid>
          </Flex>
        </>
      )}
    </>
  );
};

export default Home;
