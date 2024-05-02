import {
  Box,
  Container,
  Heading,
  FormControl,
  FormLabel,
  Input,
  InputGroup,
  InputLeftElement,
  Button,
  Flex,
  Badge,
  Textarea,
  useToast,
} from '@chakra-ui/react';

import {
  AutoComplete,
  AutoCompleteInput,
  AutoCompleteItem,
  AutoCompleteList,
  AutoCompleteCreatable,
} from '@choc-ui/chakra-autocomplete';

import { FaSearch } from 'react-icons/fa';
import { useState, useEffect } from 'react';

import axios from 'axios';

import formatService from '../../hooks/FormatService';
import productService from '../../hooks/ProductService';

const CreateProduct = () => {
  const toast = useToast();

  const [recordId, setRecordId] = useState('');
  const [formatName, setFormatName] = useState('');
  const [description, setDescription] = useState('');
  const [price, setPrice] = useState(0);
  const [quantity, setQuantity] = useState(0);
  const [location, setLocation] = useState('');
  const [restockLevel, setRestockLevel] = useState(0);
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState([]);
  const [formatSearchQuery, setFormatSearchQuery] = useState('');
  const [formatOptions, setFormatOptions] = useState([]);
  const [isFormatLoading, setIsFormatLoading] = useState(false);

  const handleSearch = async (e) => {
    const query = e.target.value;
    setSearchQuery(query);

    if (query.trim() === '') {
      setSearchResults([]);
      return;
    }

    try {
      const data = await getRecords(query);
      setSearchResults(data.results);
    } catch (error) {
      console.error('Error fetching records:', error);
    }
  };

  const getRecords = async (query) => {
    const response = await axios.get(import.meta.env.VITE_API_URL + 'records', {
      params: { title: query },
    });
    return response.data;
  };

  useEffect(() => {
    const fetchFormats = async () => {
      setIsFormatLoading(true);
      try {
        const formats = await formatService.searchFormats(formatSearchQuery);
        setFormatOptions(formats);
      } catch (error) {
        console.error('Error fetching formats:', error);
      } finally {
        setIsFormatLoading(false);
      }
    };

    fetchFormats();
  }, [formatSearchQuery]);

  const handleCreateProduct = async () => {
    const newProduct = {
      recordId,
      formatName,
      description,
      price,
      quantity,
      location,
      restockLevel,
    };

    try {
      console.log('New Product:', newProduct);
      const response = await productService.createProduct(newProduct);

      toast({
        title: 'Продукт додано',
        description: `Продукт ${response.id} успішно додано`,
        status: 'success',
        duration: 5000,
        isClosable: true,
      });
    } catch (error) {
      console.error('Error creating product:', error);
      if (error.response.status === 400) {
        toast({
          title: 'Помилка',
          description: 'Перевірте правильність введених даних',
          status: 'error',
          duration: 5000,
          isClosable: true,
        });
        return;
      }
      toast({
        title: 'Помилка',
        description: 'Помилка при додаванні продукту. ' + error.message,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }
  };

  return (
    <Box bg='gray.100' py={12} flexGrow={1}>
      <Container maxW='7xl'>
        <Heading color='teal.600' mb={4}>
          Додати продукт
        </Heading>
        <Flex mb={4}>
          <InputGroup mr={4}>
            <InputLeftElement pointerEvents='none'>
              <FaSearch color='gray.300' />
            </InputLeftElement>
            <Input
              type='text'
              placeholder='Пошук записів'
              value={searchQuery}
              onChange={handleSearch}
            />
          </InputGroup>

          <AutoComplete
            openOnFocus
            isLoading={isFormatLoading}
            mb={4}
            creatable
            onChange={(value) => setFormatName(value)}
          >
            <AutoCompleteInput
              placeholder='Формат'
              onChange={(e) => {
                setFormatSearchQuery(e.target.value);
              }}
            />
            <AutoCompleteList>
              {formatOptions.map((option, index) => (
                <AutoCompleteItem key={index} value={option.name}>
                  {option.name}
                </AutoCompleteItem>
              ))}
              <AutoCompleteCreatable />
            </AutoCompleteList>
          </AutoComplete>

          {/* <Select
            placeholder='Формат'
            value={formatId}
            onChange={(e) => setFormatId(e.target.value)}
            isLoading={isFormatLoading}
          >
            {formatOptions.map((option) => (
              <option key={option.id} value={option.id}>
                {option.name}
              </option>
            ))}
          </Select> */}
        </Flex>
        <Box overflowY='auto' maxH='300px' mb={4}>
          {searchResults.map((record) => (
            <Flex
              key={record.id}
              align='center'
              mb={2}
              p={4}
              bg='white'
              borderRadius='md'
              boxShadow='md'
            >
              <Box flex='1' mr={4}>
                <Heading size='md' color='teal.600' mb={2}>
                  {record.title}
                </Heading>
                <Box>
                  <Badge colorScheme='yellow' mr={2}>
                    {record.releaseDate.slice(0, 4)}
                  </Badge>
                  <Badge colorScheme='teal' mr={2}>
                    ID: {record.id}
                  </Badge>
                </Box>
              </Box>
              <Button
                colorScheme='teal'
                onClick={() => {
                  setRecordId(record.id);
                  setSearchQuery('');
                  setSearchResults([]);
                }}
                _hover={{ bg: 'teal.700' }}
                transition='background-color 0.3s ease'
              >
                Вибрати
              </Button>
            </Flex>
          ))}
        </Box>
        <Box>
          <Heading size='md' color='teal.600' mb={2}>
            Вибрано: <Badge colorScheme='teal'>{recordId}</Badge>
          </Heading>
        </Box>
        <FormControl mb={4}>
          <FormLabel>Опис</FormLabel>
          <Textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
        </FormControl>
        <FormControl mb={4}>
          <FormLabel>Ціна</FormLabel>
          <Input
            type='number'
            value={price}
            onChange={(e) => setPrice(parseFloat(e.target.value))}
            min={0}
            step='0.01'
          />
        </FormControl>
        <FormControl mb={4}>
          <FormLabel>Кількість</FormLabel>
          <Input
            type='number'
            value={quantity}
            onChange={(e) => setQuantity(parseInt(e.target.value, 10))}
            min={0}
          />
        </FormControl>
        <FormControl mb={4}>
          <FormLabel>Локація на складі</FormLabel>
          <Input
            value={location}
            onChange={(e) => setLocation(e.target.value)}
          />
        </FormControl>
        <FormControl mb={4}>
          <FormLabel>Рівень поповнення</FormLabel>
          <Input
            type='number'
            value={restockLevel}
            onChange={(e) => setRestockLevel(parseInt(e.target.value, 10))}
            min={0}
          />
        </FormControl>
        <Button
          colorScheme='teal'
          mt={4}
          onClick={handleCreateProduct}
          _hover={{ bg: 'teal.700' }}
          transition='background-color 0.3s ease'
        >
          Створити продукт
        </Button>
      </Container>
    </Box>
  );
};

export default CreateProduct;
