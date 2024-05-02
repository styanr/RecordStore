import {
  Box,
  Container,
  Heading,
  Text,
  Flex,
  Spinner,
  Divider,
  Badge,
} from '@chakra-ui/react';
import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import useArtists from '../hooks/useArtists';
import useRecords from '../hooks/useRecords';
import productService from '../hooks/ProductService';
import formatDate from '../utils/formatDate';

import { Link } from 'react-router-dom';

const Artist = () => {
  const { id } = useParams();
  const { getArtist } = useArtists();
  const { getRecordsForArtist } = useRecords();
  const [artist, setArtist] = useState(null);
  const [records, setRecords] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [products, setProducts] = useState([]);
  useEffect(() => {
    const fetchData = async () => {
      setIsLoading(true);
      const artistData = await getArtist(id);
      const recordsData = await getRecordsForArtist(id);

      await Promise.all(
        recordsData.map(async (record) => {
          const productsData = await productService.getProductForRecord(
            record.id
          );
          setProducts((prevProducts) => ({
            ...prevProducts,
            [record.id]: productsData,
          }));
        })
      );

      setArtist(artistData);
      setRecords(recordsData);
      setIsLoading(false);
    };

    fetchData();
  }, []);

  if (isLoading) {
    return (
      <Box bg='gray.100' py={12} h='full'>
        <Container maxW='7xl'>
          <Flex justify='center' mt={8}>
            <Spinner size='xl' color='teal.500' />
          </Flex>
        </Container>
      </Box>
    );
  }

  return (
    <Box bg='gray.100' py={12} flexGrow={1}>
      <Container maxW='7xl'>
        <Heading color='teal.600' mb={4}>
          {artist.name}
        </Heading>
        <Text mb={8}>{artist.description}</Text>
        <Divider mb={8} />
        <Heading size='md' color='teal.600' mb={4}>
          Записи
        </Heading>
        {records.length === 0 ? (
          <Text>Немає записів для цього виконавця</Text>
        ) : (
          <Box>
            {records.map((record) => (
              <Box key={record.id} mb={8}>
                <Heading size='md' mb={2}>
                  {record.title}
                </Heading>
                <Text mb={2}>Реліз: {formatDate(record.releaseDate)}</Text>
                <Text mb={4}>{record.description}</Text>
                <Flex wrap='wrap'>
                  {products[record.id].map((product) => (
                    <Badge
                      key={product.id}
                      colorScheme='teal'
                      mr={2}
                      mb={2}
                      px={3}
                      py={1}
                      as={Link}
                      to={`/products/${product.id}`}
                      _hover={{ transform: 'scale(1.05)' }}
                      transition={'all 0.3s'}
                      fontSize='sm'
                    >
                      {product.format.name}
                    </Badge>
                  ))}
                </Flex>
              </Box>
            ))}
          </Box>
        )}
      </Container>
    </Box>
  );
};

export default Artist;
